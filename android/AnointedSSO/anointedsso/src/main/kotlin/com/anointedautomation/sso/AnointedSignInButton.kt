// Copyright © Anointed Automation Corporation., 2026. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me Jesus is King ✝️

package com.anointedautomation.sso

import androidx.compose.foundation.background
import androidx.compose.foundation.clickable
import androidx.compose.foundation.layout.Arrangement
import androidx.compose.foundation.layout.Row
import androidx.compose.foundation.layout.defaultMinSize
import androidx.compose.foundation.layout.fillMaxWidth
import androidx.compose.foundation.layout.padding
import androidx.compose.foundation.layout.size
import androidx.compose.foundation.shape.RoundedCornerShape
import androidx.compose.material3.Text
import androidx.compose.runtime.Composable
import androidx.compose.ui.Alignment
import androidx.compose.ui.Modifier
import androidx.compose.ui.draw.clip
import androidx.compose.ui.graphics.Color
import androidx.compose.ui.semantics.Role
import androidx.compose.ui.text.font.FontWeight
import androidx.compose.ui.unit.dp
import androidx.compose.foundation.layout.Box

/**
 * The standard "Sign in with Anointed Automation" button (black, white text, 8dp corners, 44dp tall).
 * Compose is an optional dependency: add it to your app to use this. The logo is live at
 * https://www.anointedautomation.net/favicon.png; pass it through [logo] with your image loader
 * (the kit adds no image dependency).
 */
@Composable
fun AnointedSignInButton(
    onClick: () -> Unit,
    modifier: Modifier = Modifier,
    title: String = "Sign in with Anointed Automation",
    logo: (@Composable () -> Unit)? = null,
) {
    Row(
        modifier = modifier
            .fillMaxWidth()
            .defaultMinSize(minHeight = 44.dp)
            .clip(RoundedCornerShape(8.dp))
            .background(Color.Black)
            .clickable(role = Role.Button, onClick = onClick)
            .padding(horizontal = 16.dp),
        horizontalArrangement = Arrangement.Center,
        verticalAlignment = Alignment.CenterVertically,
    ) {
        if (logo != null) {
            Box(modifier = Modifier.size(20.dp)) { logo() }
            Box(modifier = Modifier.size(10.dp))
        }
        Text(text = title, color = Color.White, fontWeight = FontWeight.SemiBold)
    }
}
